import { OnInit, Component, computed, signal, inject, effect } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CurrencyPipe, TitleCasePipe } from '@angular/common';

import { EmpCard } from '../emp-card/emp-card';
import { SalaryRangePipe } from '../../pipes/salary-range-pipe';
import { EmployeeServices } from '../../services/employee';
import { Employee } from '../../models/employee/employee-module';
import { max } from 'rxjs';

@Component({
  imports: [FormsModule, CurrencyPipe, TitleCasePipe, SalaryRangePipe, EmpCard],
  selector: 'app-dashboard',
  styleUrl: './dashboard.css',
  templateUrl: './dashboard.html',
})

export class Dashboard implements OnInit {

  searchText = signal('');
  employeeCount = computed(() => this.EmpList().length);
  private employeeServices = inject(EmployeeServices); //Which Object it creates?
  EmpList = this.employeeServices.employees;
  filteredEmployees = computed(() => this.EmpList().filter(emp => emp.name.toLowerCase().includes(this.searchText().toLowerCase())));

  newEmployee: Employee = {
    "id": 0,
    "name": "",
    "email": "",
    "designation": "",
    "managerId": null,
    "salary": 0,
    "isActive": true,
    "departmentId": 1,
};
  formMessage: string = '';

  ngOnInit(): void {
    this.employeeServices.getEmployees().subscribe({
      next: (employees) => {
        this.employeeServices.setEmployees(employees);
        console.log("Employees Loaded");
      },
      error: (error) => {
        console.error("Failed to load: ", error);
      }
    });
  }

  addEmployee(): void {
    if (!this.newEmployee.name.trim() || !this.newEmployee.designation.trim()) {
      this.formMessage = 'Please fill in all fields correctly.';
      return;
    }
     const added = this.employeeServices.addEmployee(this.newEmployee);
     if (!added) {
       this.formMessage = 'Employee Already Exists';
       return;
     }

     else {
       this.newEmployee = { id: 0, name: '', designation: '', salary: 0, isActive: true, email: '', managerId: 0, departmentId: 0 };
       this.formMessage = 'Employee added successfully.';
       return;
     }
   }

  removeEmployee(id: number): void {
    this.employeeServices.removeEmployee(id);
  }

  constructor() {
    effect(() => { console.log('Total Employees:', this.employeeCount()); });
   }
}

//Delay, Rebounds, Retry with best practices

