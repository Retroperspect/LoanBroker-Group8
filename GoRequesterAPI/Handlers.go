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
	w.Header().Set("Content-Type", "text/plain")

	w.Header().Set("Access-Control-Allow-Headers", "Access-Control-Allow-Origin ,Accept, Content-Type, Content-Length, Accept-Encoding")
	if origin := r.Header.Get("Origin"); origin != "" {
		w.Header().Set("Access-Control-Allow-Origin", origin)
	} else { w.Header().Set("Access-Control-Allow-Origin", "*")}
	SendEnriched(w, r)

	doLongOperation(w, r)



}