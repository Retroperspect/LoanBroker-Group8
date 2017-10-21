import { LoanResponse } from './loan-response.model';
import { Injectable } from '@angular/core';
//import { Http } from '@angular/http';
import {Http, Headers, RequestOptions,Response} from '@angular/http';


@Injectable()
export class LoanService {

    constructor(private http: Http) {

    }
    postLoan(s): Promise<string> {
	let headers = new Headers();
	headers.append('Content-Type', 'text/plain');
	//var s = {"ssn": "170494-1837", "amount": 200, "term": 650};
	var data = JSON.stringify(s);
	let options = new RequestOptions({ headers: headers });
        return this.http.post('http://165.227.151.217:8989/request',data, options)
                .toPromise()
                //.then(response => response.json().data as LoanResponse[] );
				.then( response => { 
		console.log( response.json() );
		return response.json();
		});
    }
    

}