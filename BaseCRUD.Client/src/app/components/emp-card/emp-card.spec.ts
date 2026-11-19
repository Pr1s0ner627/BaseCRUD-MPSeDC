import { ComponentFixture, TestBed } from '@angular/core/testing';
import { EmpCard } from './emp-card';

describe('EmpCard', () => {
  let component: EmpCard;
  let fixture: ComponentFixture<EmpCard>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EmpCard],
    }).compileComponents();

    fixture = TestBed.createComponent(EmpCard);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
