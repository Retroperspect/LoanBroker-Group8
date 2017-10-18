package main

import (
	"fmt"
	"net/http"
)

func Index(w http.ResponseWriter, r *http.Request) {
	fmt.Fprintln(w, "Welcome!, you can acces the Request API from ip:port/request \n api accepts json like this: \n {''ssn'':<string>,''amount'':<int>,''term'':<int>} \n this will return json like this: \n {''ssn'':<string>,''interestrate'':<float>,''bank'':<string>}")
}

func RequestLoan(w http.ResponseWriter, r *http.Request){
	w.WriteHeader(http.StatusOK)

	SendEnriched(w, r)

	doLongOperation(w, r)



}