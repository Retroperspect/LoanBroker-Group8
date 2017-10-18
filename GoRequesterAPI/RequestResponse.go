package main

type LoanRequest struct {
	Ssn string `json:"ssn"`
	Amount int `json:"amount"`
	Term int `json:"term"`
}

type LoanResponse struct {
	Ssn string `json:"ssn"`
	Interestrate float32 `json:"interestrate"`
	Bank string `json:"bank"`

}