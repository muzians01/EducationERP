import { CurrencyPipe, DatePipe } from '@angular/common';
import { Component, inject } from '@angular/core';

import { AppDataStore } from '../app.data';

@Component({
  selector: 'app-fees-page',
  imports: [DatePipe, CurrencyPipe],
  template: `
    <section class="section-heading">
      <p class="eyebrow">Fees Module</p>
      <h2>Balances, concessions, and receipts</h2>
    </section>

    <div class="workspace-grid">
      <article class="data-card">
        <div class="data-card__header">
          <div>
            <p class="eyebrow">Outstanding</p>
            <h3>Students pending collection</h3>
          </div>
        </div>
        @if (store.feesDashboard(); as fees) {
          <div class="application-list">
            @for (fee of fees.outstandingFees; track fee.id) {
              <article class="application-item">
                <div class="application-item__main">
                  <div>
                    <strong>{{ fee.studentName }}</strong>
                    <p>{{ fee.feeName }} - {{ fee.admissionNumber }}</p>
                  </div>
                  <span class="status-chip" [class]="'status-chip status-chip--' + statusTone(fee.status)">{{ fee.status }}</span>
                </div>
                <div class="application-meta">
                  <span>{{ fee.dueOn | date: 'dd MMM yyyy' }}</span>
                  <span>Concession {{ fee.concessionAmount | currency: 'INR':'symbol':'1.0-0' }}</span>
                  <span>Balance {{ fee.balanceAmount | currency: 'INR':'symbol':'1.0-0' }}</span>
                </div>
              </article>
            }
          </div>
        }
      </article>

      <article class="data-card">
        <div class="data-card__header">
          <div>
            <p class="eyebrow">Receipts</p>
            <h3>Recent posted collections</h3>
          </div>
        </div>
        @if (store.feesDashboard(); as fees) {
          <div class="guardian-list">
            @for (receipt of fees.recentReceipts; track receipt.id) {
              <article class="guardian-item">
                <strong>{{ receipt.studentName }}</strong>
                <p>{{ receipt.feeName }} - {{ receipt.paymentMethod }}</p>
                <p>{{ receipt.receiptNumber }} - {{ receipt.amount | currency: 'INR':'symbol':'1.0-0' }}</p>
                <span>{{ receipt.paidOn | date: 'dd MMM yyyy' }}</span>
              </article>
            }
          </div>
        }
      </article>

      <article class="data-card">
        <div class="data-card__header">
          <div>
            <p class="eyebrow">Summary</p>
            <h3>Net receivable view</h3>
          </div>
        </div>
        @if (store.feesDashboard(); as fees) {
          <div class="guardian-list">
            <article class="guardian-item">
              <strong>Expected</strong>
              <p>{{ fees.totalExpectedAmount | currency: 'INR':'symbol':'1.0-0' }}</p>
              <span>Before concessions</span>
            </article>
            <article class="guardian-item">
              <strong>Concession</strong>
              <p>{{ fees.totalConcessionAmount | currency: 'INR':'symbol':'1.0-0' }}</p>
              <span>Approved reductions</span>
            </article>
            <article class="guardian-item">
              <strong>Net Receivable</strong>
              <p>{{ fees.netReceivableAmount | currency: 'INR':'symbol':'1.0-0' }}</p>
              <span>After concessions</span>
            </article>
          </div>
        }
      </article>
    </div>
  `
})
export class FeesPageComponent {
  protected readonly store = inject(AppDataStore);
  protected readonly statusTone = (status: string) => status.toLowerCase().replace(/\s+/g, '-');
}
