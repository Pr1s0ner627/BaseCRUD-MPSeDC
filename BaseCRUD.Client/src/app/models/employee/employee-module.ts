import { isActive } from "@angular/router";

export interface Employee {
  id: number;
  name: string;
  email: string;
  designation: string;
  salary: number | null;
  managerId: number | null;
  isActive: boolean;
  departmentId: number;
}

