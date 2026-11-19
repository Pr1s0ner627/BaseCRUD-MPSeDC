import { Injectable, signal, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Employee } from '../models/employee/employee-module';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class EmployeeServices {

  private http = inject(HttpClient);
  private apiURL = 'https://localhost:7275/Employee';
  private _employees = signal<Employee[]>([]);

  employees = this._employees.asReadonly(); 

  getEmployees(): Observable<Employee[]> {
    return this.http.get<Employee[]>(this.apiURL);
  }

  setEmployees(employees: Employee[]): void {
    this._employees.set(employees);
  }

  addEmployee(employee: Employee): boolean {
    const duplicate = this._employees().some(emp => emp.name === employee.name && emp.designation === employee.designation);
    if (duplicate) {
      throw new Error("Employee with the same name and designation already exists");
    }
    this._employees.update(employees => [...employees, {...employee}]);
    return true;
  }

  removeEmployee(id: number): void {
    this._employees.update(employees => employees.filter(emp => emp.id !== id));
  }
}
