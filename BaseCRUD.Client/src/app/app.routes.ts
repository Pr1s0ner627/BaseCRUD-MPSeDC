import { Routes } from '@angular/router';
import { Dashboard } from './components/dashboard/dashboard';

export const routes: Routes = [
  {
    path: '',
    component: Dashboard
  },
  {
    path: 'authorization',
    loadComponent: () => import('./components/authorization/authorization').then(m => m.Authorization)
  },
  {
    path: 'register',
    loadComponent: () => import('./components/authorization/register/register').then(m => m.Register)
  }
 ];
