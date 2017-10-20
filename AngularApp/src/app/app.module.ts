import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';


import { AppComponent } from './app.component';
import { LoanFormComponent, LoanService } from './loan-form';

@NgModule({
  declarations: [
    AppComponent,
    LoanFormComponent
	
	
  ],
  imports: [
    BrowserModule,
	HttpModule,
	FormsModule
  ],
  providers: [LoanService],
  bootstrap: [AppComponent]
})
export class AppModule { }
