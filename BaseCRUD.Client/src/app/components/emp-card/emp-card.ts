import { Component, input, output } from '@angular/core';

@Component({
  imports: [],
  selector: 'app-emp-card',
  styleUrl: './emp-card.css',
  templateUrl: './emp-card.html',
})
export class EmpCard {
  empName = input.required<string>();
  remove = output<string>();
}
