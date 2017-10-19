package main

import (
	"log"
	"encoding/xml"
	"github.com/streadway/amqp"
	"fmt"
)


//////////////////////////// REMEMBER TO IMPORT
////////////////////////////  go get github.com/streadway/amqp


////This functions checks if the error actully has an error that is fatal and prints it out.
func failOnError(err error, msg string) {
	if err != nil {
		log.Fatalf("%s: %s", msg, err)
		panic(fmt.Sprintf("%s: %s", msg, err))
	}
}

type Request struct {
	Ssn string `xml:"ssn"`
	Creditscore int `xml:"creditScore"`
	Loanamount float32 `xml:"loanAmount"`
	Loanduration string `xml:"loanDuration"`


}


type LoanResponse struct {
	XMLName xml.Name `xml:"loanResponse"`
	Ssn string `xml:"ssn"`
	InterestRate float32 `xml:"interestRate"`
}




func SerializeToXML(response LoanResponse) []byte {


	XMLString,_ := xml.MarshalIndent(response,"","   ")


	return XMLString
}


func SendResponse(Req Request, header amqp.Table, corID string){
	log.Println("Succesfully received message nad transformed message to:",Req.Ssn,Req.Creditscore,Req.Loanamount,Req.Loanduration)
	var response LoanResponse
	response.Ssn = Req.Ssn
	if Req.Creditscore > 750{
		response.InterestRate = 13
	}else {
		response.InterestRate = 10
	}





	conn, err := amqp.Dial("amqp://admin:password@138.197.186.82")
	failOnError(err, "Failed to connect to RabbitMQ")
	defer conn.Close()

	ch, err := conn.Channel()
	failOnError(err, "Failed to open a channel")
	defer ch.Close()

	q, err := ch.QueueDeclare(
		"GoBankResponse", // name
		true,         // durable
		false,        // delete when unused
		false,        // exclusive
		false,        // no-wait
		nil,          // arguments
	)
	failOnError(err, "Failed to declare a queue")



	body := SerializeToXML(response)
	err = ch.Publish(
		"",           // exchange
		q.Name,       // routing key
		false,        // mandatory
		false,
		amqp.Publishing{
			DeliveryMode: amqp.Persistent,
			ContentType:  "text/plain",
			Body:         body,
			Headers:		header,
			CorrelationId:	corID,


		})
	failOnError(err, "Failed to publish a message")
	log.Printf(" [x] Sent %s", body)

}


func main() {

	////// Defines connection and a potential error with a ampq dial to a rabbitMQserver.
	conn, err := amqp.Dial("amqp://admin:password@138.197.186.82")
	////// Checks if error is fatal.
	failOnError(err, "Failed to connect to RabbitMQ")
	///// will close connection so it is not constantly connected.
	defer conn.Close()

	///// Defines a channel and a potential error by the connection.channel() method.
	ch, err := conn.Channel()
	////// Checks error if it is fatal.
	failOnError(err, "Failed to open a channel")
	//// Will close connection.
	defer ch.Close()

	err = ch.ExchangeDeclare("GoBankRequest", "fanout", true, false, false, false, nil,	)
	failOnError(err, "Failed to declare an exchange")

	//// Defines a que and a potential error by the ch.QueueDeclare)
	q, err := ch.QueueDeclare(
		"", // name
		true,         // durable
		false,        // delete when unused
		false,        // exclusive
		false,        // no-wait
		nil,          // arguments
	)
	failOnError(err, "Failed to declare a queue")

	err = ch.Qos(
		1,     // prefetch count
		0,     // prefetch size
		false, // global
	)
	failOnError(err, "Failed to set QoS")


	err = ch.QueueBind(q.Name, "", "GoBankRequest", false,nil)
	failOnError(err, "Failed to bind a que")
	//// Defines the messages and a potential error when consuming the message.
	msgs, err := ch.Consume(
		q.Name, // queue
		"",     // consumer
		false,  // auto-ack
		false,  // exclusive
		false,  // no-local
		false,  // no-wait
		nil,    // args
	)
	failOnError(err, "Failed to register a consumer")

	forever := make(chan bool)

	go func() {
		for d := range msgs {
			log.Printf("Received a message: %s", d.Body)

			//// What to do when consuming message.
			XMLMSG := d.Body;
			Header := d.Headers
			CorID := d.CorrelationId


			var req Request
			err := xml.Unmarshal([]byte(XMLMSG), &req)
			failOnError(err, "Failed to Unmarshal XML-Message")
			SendResponse(req, Header, CorID)
			log.Printf("Done")
			d.Ack(false)
		}
	}()

	log.Printf(" [*] Waiting for messages. To exit press CTRL+C")
	<-forever
}



