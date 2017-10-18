package main

import (
	"net/http"
	"encoding/json"
	"github.com/streadway/amqp"
	"log"
	"io/ioutil"
	"io"
)

func doLongOperation(w http.ResponseWriter, r *http.Request){

	/// Establish Connection.
	conn, err := amqp.Dial("amqp://admin:password@138.197.186.82"); if err != nil { panic(err) }
	defer conn.Close()

	ch, err := conn.Channel(); if err != nil { panic(err) }
	defer ch.Close()




	// Receive Message


	q, err := ch.QueueDeclare(
		"BestResponse",
		true,
		false,
		false,
		false,
		nil,
	); if err != nil { panic(err)}

	msgs, err := ch.Consume(
		q.Name,
		"",
		true,
		false,
		false,
		false,
		nil,
	); if err != nil { panic(err)}



		forever := make(chan bool)
		go func() {
			for d := range msgs {
				log.Printf("Received a message: %s", d.Body)

				w.Write(d.Body)
				log.Println("Now it should be done")
				forever <- true

			}
		}()
		log.Println(" Expecting Message, waiting for that.")
		<-forever
		log.Println("Done Listening!")

}

func SendEnriched(w http.ResponseWriter, r *http.Request){
	conn, err := amqp.Dial("amqp://admin:password@138.197.186.82"); if err != nil { panic(err) }
	defer conn.Close()

	ch, err := conn.Channel(); if err != nil { panic(err) }
	defer ch.Close()

	var req LoanRequest
	// Send Message
	q1, err := ch.QueueDeclare(
		"RequestAPI",
		true,
		false,
		false,
		false,
		nil,
	); if err != nil { panic(err)}

	body, err := ioutil.ReadAll(io.LimitReader(r.Body, 1048576))
	if err != nil {
		panic(err)
	}
	if err := r.Body.Close(); err != nil {
		panic(err)
	}
	if err := json.Unmarshal(body, &req); err != nil {
		w.Header().Set("Content-Type", "application/json; charset=UTF-8")
		w.WriteHeader(422)
		if err := json.NewEncoder(w).Encode(err); err != nil{
			panic(err)
		}
	}

	err = ch.Publish(
		"",     // exchange
		q1.Name, // routing key
		false,  // mandatory
		false,  // immediate
		amqp.Publishing{
			ContentType: "application/json",
			Body:        []byte(body),
		})
	log.Printf(" [x] Sent %s", body)
	if err != nil { panic(err)}
}
