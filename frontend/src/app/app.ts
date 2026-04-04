import { Component, inject, ViewEncapsulation } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';

import { AppDataStore } from './app.data';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './app.html',
  styleUrl: './app.scss',
  encapsulation: ViewEncapsulation.None
})
export class App {
  protected readonly title = 'Education ERP Workspace';
  protected readonly navItems = [
    { label: 'Overview', path: '/' },
    { label: 'Admissions', path: '/admissions' },
    { label: 'Academics', path: '/academics' },
    { label: 'Examinations', path: '/examinations' },
    { label: 'Fees', path: '/fees' },
    { label: 'Attendance', path: '/attendance' }
  ];
  private readonly store = inject(AppDataStore);

  constructor() {
    this.store.load();
  }
}
