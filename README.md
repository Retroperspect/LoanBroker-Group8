# LoanBroker Project
This is the project: Loan Broker project.

Credit: Emmely(cph-el69), Kristian(cph-kf96), Daniel(cph-dh136).

This project is based of a project description, which can be found in the link below:
- [Project-Link](https://github.com/datsoftlyngby/soft2017fall-system-integration-teaching-material/blob/master/assignments/LoanBrokerProject.pdf)

## LoanBroker System
The product of this project is a system that can take in loan requests, then contact a number of banks, which then individually gives back a response to the system, The system then determines the best loan plan and relays that to the user. The system consists of alot of sub systems which needs to be integrated. A system for looking up a persons creditscore, a system that determines which banks would accept the request based on creditscore, a systems to translate requests and contact the banks, a system that can normalize the responses, a system that can combine/aggregate all the responses for every request, a system that determines the best response, and of cause a system that a user can request loans from. As described, alot of sub systems are at play in this project. we've chosen 2 ways of integrating all these systems together. The main way is by messaging, and the other way is with remote procedure invocation. Other ways to integrate systems, which we thought was not relevant or smart in our case is: File transfer and shared database.

## Integrating systems
As mentined before, we have chosen 2 ways of integrating systems. By messaging and remote procedure invocations. 

#### Messaging
For messaging we have used RabbitMQ as our messagin broker. RabbitMQ have a wide range of languages that it can be used with. In our case we have used rabbitmq in C# and Go systems. We have setup our own RabbitMQ server on a Linux(Ubuntu16.04) Digital ocean droplet.
```
http://138.197.186.82:15672
```
This server is mostly used when we are doing messaging. But a few systems have not been build by us and are not in contact with our RabbitMQ server, which is why we are also implementing another RabbitMQ server only for 2 systems which are 2 banks build by cphbusiness.
```
datdb.cphbusiness.dk:15672
```

#### Remote procedure invocation
We are also using Simple Object Acces Protocoll (SOAP) to integrate. We have 3 systems which integrate with this way which are: The system that determines creditscore, the system that determines which banks will accept the request, a bank that handles requests.

## Systems
This section will list and give an overview of all the systems that are integrated to become a complete LoanBroker.

LoanBroker System overview: [Overview][1].

Screendumps of running code: [Screendump][2].

Message transformations through the system: [Message Transformation](Documentation/MessageTransform.md).

List of all systems:
- Website:
```
Hosted on: AWS - IIS = IP:
Programmed in: Angular 2
Github folder: AngularApp
Responseability: To handle user interaction (Front-End of LoanBroker)
Integration Method: RPI (restfull web api: Go Request API)
```
- Go Request API
``` 
Hosted on: Digital Ocean - DockerContainer(go) = IP: http://165.227.151.217:8989/
Programmed in: Go
Github folder: GoRequesterAPI
Responseability: Interface for Website to communicate with LoanBroker backend.
Integration Method: Messaging
```
- Loaner
``` 
Hosted on: Locahost 
Programmed in: C# (Console app)
Github folder: Loaner/Loaner/
Responseability: To start a Loan Request in LoanBroker (Translates)
Integration Method: Messaging
```
- GetCreditScore
``` 
Hosted on: Localhost
Programmed in: C# (Console app)
Github folder: Loaner/GetCreditScore/
Responseability: To contact the Credit Bureau and enrich request with creditscore.
Integration Method: Messaging
```
- Credit Bureau
``` 
Responseability: To determine the creditscore of the user that are requesting
Integration Method: RPI (SOAP)
```
- GetBanks
``` 
Hosted on: Localhost
Programmed in: C# (Console app)
Github folder: Loaner/GetBanks/
Responseability: To contact GetBanks Rulebase and enrich the request with this information.
Integration Method: Messaging
```
- GetBanks - Soapservice
``` 
Hosted on: Local - IIS = IP: Localhost:53156
Programmed in: C# (WCF / Asp.net)
Github folder: GetBanks - Soapservice/
Responseability: To Determine which banks are egible to send the request to.
Integration Method: RPI (SOAP)
```
- RecipientList-Router
``` 
Hosted on: Localhost
Programmed in: C# (Console app)
Github folder: Loaner/RecipientList-Router/
Responseability: To Route the requests to all egible bank translators.
Integration Method: Messaging
```
- XML Translator
``` 
Hosted on: Localhost
Programmed in: C# (Console app)
Github folder: Loaner/XMLTranslator/
Responseability: To translate the request to the banks prefered format and then send request to bank.
Integration Method: Messaging
```
- JSON Translator
``` 
Hosted on: Localhost
Programmed in: C# (Console app)
Github folder: Loaner/JSONTranslator/
Responseability: To translate the request to the banks prefered format and then send request to bank.
Integration Method: Messaging
```
- WebBank Translator
``` 
Hosted on: Localhost
Programmed in: C# (Console app)
Github folder: Loaner/WebBankConsole/
Responseability: To translate the request to the banks prefered format and then send request to bank.
Integration Method: Messaging
```
- GoBank
``` 
Hosted on: Localhost
Programmed in: Go
Github folder: GoBank
Responseability: To handle requests and respond with a loan response
Integration Method: Messaging
```
- WebBank
``` 
Hosted on: Local - IIS = IP: Localhost:51448
Programmed in: C# (WCF / Asp.net)
Github folder: Loaner/WebBankConsole/
Responseability: To handle requests and respond with a loan response
Integration Method: RPI (SOAP)
```
- CPHXML Bank
``` 
Responseability: To handle requests and respond with a loan response
Integration Method: Messaging
```
- CPHJSON Bank
``` 
Responseability: To handle requests and respond with a loan response
Integration Method: Messaging
```
- Normaliser
``` 
Hosted on: Localhost
Programmed in: C# (Console app)
Github folder: Loaner/Normalizer/
Responseability: To normalise all the responses from all the different banks to a universal format the LoanBroker uses
Integration Method: Messaging
```
- Aggregator
``` 
Hosted on: Localhost
Programmed in: C# (Console app)
Github folder: Loaner/Aggregator/
Responseability: To aggregate all the responses from the banks to one response with all the banks responses in it.
Integration Method: Messaging
```
- Determiner
``` 
Hosted on: Localhost
Programmed in: C# (Console app)
Github folder: Loaner/Determiner/
Responseability: To determine the best response
Integration Method: Messaging
```

## Potential bottlenecks
This section is to describe potential bottlenecks in our solution and what could be done to enhance the performance.

Since the system was not designed with multi threading or even load balancing, it's possible the aggregator would be overloaded with responses that it won't be able to handle to a certain level, although it does have the ability to handle multiple different loan requests with it's inbuild functions in the rabbitMQ client, it's not custom tailored to the specific requirements of the system, and since we havn't done extensive stress test, it's unknown how well it would be able to handle if incase more messages than the system can handle in time, as it could result in messages ending in side channel of being to slow and being lost to the user, as we don't look at this channel incase of message overload.

A solution to this would be a sub system, that handles messages before the aggregator handles them, given we tag each request with a header that identify what group they belong to, we can have this middleman sub system control and redirect the messages to multiple aggregators, which would eliminate to possibility of multiple aggregators consuming eachothers messages, and also improve the performances of the unscalable aggregator sub system.

## Testability
Given the system consist of multiple sub systems, it's possible to build test cases for each individual sub system, and perform extensive load balance test, information malfunction tests and networking testing.

Incase you wish to test the system yourself, make sure to have all system running before you attempt to use the browser to do a loan request to the system.
Our setup is using visual studio to run the solution loan broker including all projects except the loaner library, also running get banks and web bank solutions in visual studio aswell, lastly start up go bank using GO statement with a terminal of your choice, we use git bash with the GO extention, to deploy GOAPI build dockerimage using dockerfile then run dockerimages and expose port 8989.
docker run -d -ti -p 8989:8989 danielhauge/loanapi:latest```

[1]:https://github.com/Retroperspect/LoanBroker-Group8/blob/master/Documentation/MessagingSystemOverview.png
[2]:https://github.com/Retroperspect/LoanBroker-Group8/blob/master/Documentation/ScreenDumps.png
