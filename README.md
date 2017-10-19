# LoanBroker Project
This is the project: Loan Broker project.

Credit: Emmely(cph-el69), Kristian(cph-kf96), Daniel(cph-dh136).

This project is based of a project description, which can be found in the link below:
- [Project-Link](https://github.com/datsoftlyngby/soft2017fall-system-integration-teaching-material/blob/master/assignments/LoanBrokerProject.pdf)

## LoanBroker System
The product of this project is a system that can take in loan requests, then contact banks which give back a response that the system, that then determines the best loan plan and then respond that to user. The system consists of alot of systems which needs to be integrated. A system for looking up a persons creditscore, a system that determines which banks would accept the request based on creditscore, systems to translate requests and contact the banks, a system that can normalize the responses, a system that can combine/aggregate all the responses for every request, a system that determines the best response, and ofcause a system that a user can request loans from. As described, alot of systems are at play in this project. we've chosen 2 ways of integrating all these systems together. The main way is by messaging, and the other way is with remote procedure invocation. Other ways to integrate systems, which we thought was not relevant or smart in our case is: File transfer and shared database.

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

LoanBroker Systemoverview: [Overview][1].

Screendumps of running code: [Screendump][2].

Message transformations through the system: [Message Transformation](Documentation/MessageTransform.md).

List of all systems:
- Website:
```
Hosted on: AWS - IIS = IP:
Programmed in: Angular 2
Github folder:
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

## Testability
This section is to describe how testable our solution is (see pp440-443)

[1]:https://github.com/Retroperspect/LoanBroker-Group8/blob/master/Documentation/MessagingSystemOverview.png
[2]:https://github.com/Retroperspect/LoanBroker-Group8/blob/master/Documentation/ScreenDumps.png
