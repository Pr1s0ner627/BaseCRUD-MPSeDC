import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'salaryRange',
  standalone: true
})
export class SalaryRangePipe implements PipeTransform {
  transform(salary: number): string {
    if (salary < 30000) {
      return '3C';
    }
    if (salary >= 30000 && salary < 50000) {
      return '2B';
    }
    return '1A';
  }
}
