import { Component, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';

import { LoanService } from './loan.service';
import { Loan } from '../loan.model';
import { LoanResponse } from './loan-response.model';


@Component({
  selector: 'app-loan-form',
  templateUrl: './loan-form.component.html',
  styleUrls: ['./loan-form.component.css']
})
export class LoanFormComponent implements OnInit {
  model = new Loan(1 ,120190 , 2233, 30000, 52);
  loanProgressing = false;
  loanFetched = false;
  //loans:LoanResponse[];
  loans:string;

  ngOnInit() {
  }
  get currentLoan() { return JSON.stringify(this.model); }
  get currentLoanResponse() { return{ ssn1:123455,term:'42' , interestrate:4.5}; }

  @ViewChild('loanForm') loanForm: NgForm;

  constructor(private loanService: LoanService) { }


   	onSubmit() {
	if (this.loanForm.invalid) return;
		console.log(this.model);
		console.log(this.model.ssn1+'-'+this.model.ssn2);
		console.log(this.model.amount);
		console.log(this.model.terms);
		this.loanForm.resetForm();
		

		/*hide form*/
		/*ToDO spinner*/
		/*show result*/
		this.loanProgressing = true;
		
		console.log('posting..');
		
		var s = {"ssn": this.model.ssn1+'-'+this.model.ssn2, "amount": this.model.amount, "term": this.model.terms};

		this.loanService.postLoan(s)
			.then( loans => { 
				console.log( 'Not called.' );
				//alert(loans)
				this.loans = loans;
				this.loanProgressing = false;
				this.loanFetched = true;
			}, reason => {
				console.error( 'onRejected function called: ', reason );
				this.loans = reason;
				this.loanProgressing = false;
				this.loanFetched = true;
		});

       
    }
}
