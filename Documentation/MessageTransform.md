# Message Transformation Overview 
This is how the Message looks through the system.

Website user starts request -> GoAPI -> RequestAPI
```
Properties	
content_type:	application/json

{"ssn": "170494-1837", "amount": 200, "term": 650}
```
-> Loaner -> RequestLoan
```
Properties	
correlation_id:	15df77c4-51ab-49ac-ae57-fa0e5095f891
delivery_mode:	2

<?xml version="1.0" encoding="utf-16"?>

<LoanRequest xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">

  <ssn>170494-1837</ssn>

  <LoanDuration>650.00:00:00</LoanDuration>

  <LoanAmmount>200</LoanAmmount>

  <CreditScore>0</CreditScore>

</LoanRequest>
```
-> GetCreditScore -> RequestWithCredit
```
Properties	
correlation_id:	15df77c4-51ab-49ac-ae57-fa0e5095f891
delivery_mode:	2

<?xml version="1.0" encoding="utf-16"?>

<LoanRequest xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">

  <ssn>170494-1837</ssn>

  <LoanDuration>650.00:00:00</LoanDuration>

  <LoanAmmount>200</LoanAmmount>

  <CreditScore>498</CreditScore>

</LoanRequest>
```
-> GetBanks -> RequestWithBanks
```
Properties	
correlation_id:	15df77c4-51ab-49ac-ae57-fa0e5095f891
delivery_mode:	2
headers:	
      Requests:	3

<?xml version="1.0" encoding="utf-16"?>

<LoanRequestWithBanks xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">

  <ssn>170494-1837</ssn>

  <LoanDuration>650.00:00:00</LoanDuration>

  <LoanAmmount>200</LoanAmmount>

  <CreditScore>498</CreditScore>

  <Bank>

    <format>XML</format>

    <Input>cphbusiness.bankXML</Input>

    <Output>Group8-LoanBroker-Request</Output>

    <BankName>CPHXML</BankName>

  </Bank>

  <Bank>

    <format>JSON</format>

    <Input>cphbusiness.bankJSON</Input>

    <Output>Group8-LoanBroker-Request</Output>

    <BankName>CPHJSON</BankName>

  </Bank>

  <Bank>

    <format>XML</format>

    <Input>GoBankRequest</Input>

    <Output>GoBankResponse</Output>

    <BankName>GoBank</BankName>

  </Bank>

</LoanRequestWithBanks>
```
-> RecipientList -> JSON, XML, (also LoanRequestB2 - But in this case, this bank was not egible)
- One to JSON:
```
Properties	
correlation_id:	15df77c4-51ab-49ac-ae57-fa0e5095f891
delivery_mode:	2
headers:	
    Requests:	3
    in:	cphbusiness.bankJSON
    reply:	Group8-LoanBroker-Request
    bname:	CPHJSON

<?xml version="1.0" encoding="utf-16"?>

<LoanRequest xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">

  <ssn>170494-1837</ssn>

  <LoanDuration>650.00:00:00</LoanDuration>

  <LoanAmmount>200</LoanAmmount>

  <CreditScore>498</CreditScore>

</LoanRequest>
```
- 2 to XML
```
Properties	
correlation_id:	15df77c4-51ab-49ac-ae57-fa0e5095f891
delivery_mode:	2
headers:	
    Requests:	3
    in:	GoBankRequest
    reply:	GoBankResponse
    bname:	GoBank

<?xml version="1.0" encoding="utf-16"?>

<LoanRequest xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">

  <ssn>170494-1837</ssn>

  <LoanDuration>650.00:00:00</LoanDuration>

  <LoanAmmount>200</LoanAmmount>

  <CreditScore>498</CreditScore>

</LoanRequest>
```
The only difference between the 2 messages, is the in, reply and bname header properties.
From here, the procedure is more or less the same for all banks, so i will only showcase one banks way. 
Translate the message(body) into the correct format, and have the "reply_to" header to tell the bank where to send the response to.

-> JSONTranslator -> cphbusiness.BANKJSON -> Group8-LoanBroker-Request
```
Properties	
reply_to:	Group8-LoanBroker-Request
correlation_id:	15df77c4-51ab-49ac-ae57-fa0e5095f891
delivery_mode:	2
headers:	
    bname:	CPHJSON
    Requests:	3
    reply:	Group8-LoanBroker-Request
    in:	cphbusiness.bankJSON

{"interestRate":9.350000000000001,"ssn":1704941837}
```
-> Normaliser -> Aggregator (Note, the normaliser listens on both our RabbitMQ response channel and Group8-LoanBroker-Request)
```
Properties	
reply_to:	Group8-LoanBroker-Request
correlation_id:	15df77c4-51ab-49ac-ae57-fa0e5095f891
delivery_mode:	2
headers:	
    bname:	CPHJSON
    Requests:	3
    reply:	Group8-LoanBroker-Request
    in:	cphbusiness.bankJSON

<?xml version="1.0" encoding="utf-16"?>

<UniversalResponse xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">

  <ssn>1704941837</ssn>

  <interestrate>9.350000000000001</interestrate>

</UniversalResponse>
```
-> Aggregator -> AllResponses (Now we only have 1 message even tho we got 3 messages, also got rid of alot of antiquated header properties)
```
Properties	
correlation_id:	15df77c4-51ab-49ac-ae57-fa0e5095f891
delivery_mode:	2

<?xml version="1.0" encoding="utf-16"?>

<Responses xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">

  <Response>

    <ssn>1704941837</ssn>

    <interestrate>9.350000000000001</interestrate>

    <bank>CPHJSON</bank>

  </Response>

  <Response>

    <ssn>1704941837</ssn>

    <interestrate>12.768</interestrate>

    <bank>CPHXML</bank>

  </Response>

  <Response>

    <ssn>1704941837</ssn>

    <interestrate>10</interestrate>

    <bank>GoBank</bank>

  </Response>

  <Aggregation_ID>15df77c4-51ab-49ac-ae57-fa0e5095f891</Aggregation_ID>

  <ExpectedResponses>3</ExpectedResponses>

</Responses>
```
-> Determiner -> BestResponse
```
Properties	
correlation_id:	15df77c4-51ab-49ac-ae57-fa0e5095f891
delivery_mode:	2

{"ssn":"1704941837","interestrate":9.350000000000001,"bank":"CPHJSON"}
```
-> GoAPI (the api then returns the best response)
