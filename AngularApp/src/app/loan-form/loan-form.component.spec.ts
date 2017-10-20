import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LoanFormComponent } from './loan-form.component';

describe('LoanFormComponent', () => {
  let component: LoanFormComponent;
  let fixture: ComponentFixture<LoanFormComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LoanFormComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LoanFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
