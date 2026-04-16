import { CurrencyPipe, DatePipe } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

import { AppDataStore } from '../app.data';
import { CreateFeeConcession, FeeConcession, FeePayment, FeeReceipt, RecordFeePayment, UpdateFeePayment } from '../app.models';

@Component({
  selector: 'app-fees-page',
  imports: [DatePipe, CurrencyPipe, ReactiveFormsModule],
  template: `
    <section class="section-heading">
      <p class="eyebrow">Fees Module</p>
      <h2>Balances, concessions, receipts, and collection flow</h2>
    </section>

    <section class="metrics">
      <article class="metric-card">
        <span>Expected</span>
        <strong>{{ (store.feesDashboard()?.totalExpectedAmount ?? 0) | currency: 'INR':'symbol':'1.0-0' }}</strong>
        <p>Gross fee billed before concessions.</p>
      </article>
      <article class="metric-card">
        <span>Collected</span>
        <strong>{{ (store.feesDashboard()?.totalCollectedAmount ?? 0) | currency: 'INR':'symbol':'1.0-0' }}</strong>
        <p>Posted collections recorded so far.</p>
      </article>
      <article class="metric-card">
        <span>Outstanding</span>
        <strong>{{ (store.feesDashboard()?.outstandingAmount ?? 0) | currency: 'INR':'symbol':'1.0-0' }}</strong>
        <p>Balance still awaiting collection.</p>
      </article>
      <article class="metric-card">
        <span>Overdue Items</span>
        <strong>{{ store.feesDashboard()?.overdueCount ?? 0 }}</strong>
        <p>Student fee rows that have crossed due date.</p>
      </article>
    </section>

    <div class="workspace-grid">
      <article class="data-card">
        <div class="data-card__header">
          <div>
            <p class="eyebrow">Payment Desk</p>
            <h3>{{ editingPaymentId() ? 'Update posted payment' : 'Post fee collection' }}</h3>
          </div>
        </div>
        <form [formGroup]="paymentForm" (ngSubmit)="onSubmitPayment()" class="form-grid">
          @if (editingPaymentId()) {
            <label class="form-field form-field--full">
              <span>Payment Record</span>
              <input type="text" [value]="selectedPaymentLabel()" readonly />
            </label>
          } @else {
            <label class="form-field">
              <span>Student Fee</span>
              <select formControlName="studentFeeId">
                <option value="">Select student fee</option>
                @for (fee of store.feesDashboard()?.outstandingFees ?? []; track fee.id) {
                  <option [value]="fee.id">{{ fee.studentName }} - {{ fee.feeName }} - {{ fee.balanceAmount | currency: 'INR':'symbol':'1.0-0' }}</option>
                }
              </select>
            </label>
          }
          <label class="form-field">
            <span>Amount</span>
            <input type="number" formControlName="amount" min="0.01" />
          </label>
          <label class="form-field">
            <span>Payment Method</span>
            <select formControlName="paymentMethod">
              <option value="">Select method</option>
              @for (method of paymentMethods; track method) {
                <option [value]="method">{{ method }}</option>
              }
            </select>
          </label>
          <label class="form-field">
            <span>Reference</span>
            <input type="text" formControlName="paymentReference" />
          </label>
          <label class="form-field">
            <span>Paid On</span>
            <input type="date" formControlName="paidOn" />
          </label>
          <div class="form-actions">
            <button type="submit" class="button button--primary" [disabled]="paymentForm.invalid || isSubmittingPayment()">
              {{ isSubmittingPayment() ? (editingPaymentId() ? 'Saving...' : 'Recording...') : (editingPaymentId() ? 'Update Payment' : 'Record Payment') }}
            </button>
            @if (editingPaymentId()) {
              <button type="button" class="button button--secondary" (click)="resetPaymentForm()">Cancel</button>
            }
          </div>
        </form>
      </article>

      <article class="data-card">
        <div class="data-card__header">
          <div>
            <p class="eyebrow">Concession Desk</p>
            <h3>Grant fee reduction</h3>
          </div>
        </div>
        <form [formGroup]="concessionForm" (ngSubmit)="onSubmitConcession()" class="form-grid">
          <label class="form-field">
            <span>Student Fee</span>
            <select formControlName="studentFeeId">
              <option value="">Select student fee</option>
              @for (fee of store.feesDashboard()?.outstandingFees ?? []; track fee.id) {
                  <option [value]="fee.id">{{ fee.studentName }} - {{ fee.feeName }}</option>
              }
            </select>
          </label>
          <label class="form-field">
            <span>Concession Type</span>
            <select formControlName="concessionType">
              <option value="">Select type</option>
              @for (type of concessionTypes; track type) {
                <option [value]="type">{{ type }}</option>
              }
            </select>
          </label>
          <label class="form-field">
            <span>Amount</span>
            <input type="number" formControlName="amount" min="0.01" />
          </label>
          <label class="form-field">
            <span>Remarks</span>
            <textarea formControlName="remarks" rows="2"></textarea>
          </label>
          <div class="form-actions">
            <button type="submit" class="button button--primary" [disabled]="concessionForm.invalid || isSubmittingConcession()">
              {{ isSubmittingConcession() ? 'Saving...' : 'Create Concession' }}
            </button>
          </div>
        </form>
      </article>

      <article class="data-card">
        <div class="data-card__header">
          <div>
            <p class="eyebrow">Approval Desk</p>
            <h3>{{ editingConcessionId() ? 'Assign approver and confirm' : 'Select a concession to approve' }}</h3>
          </div>
        </div>
        <form [formGroup]="approvalForm" (ngSubmit)="onApproveConcession()" class="form-grid">
          <label class="form-field form-field--full">
            <span>Selected Concession</span>
            <input type="text" [value]="selectedConcessionLabel()" readonly />
          </label>
          <label class="form-field">
            <span>Approved By</span>
            <input type="text" formControlName="approvedBy" />
          </label>
          <div class="form-actions">
            <button type="submit" class="button button--primary" [disabled]="approvalForm.invalid || !editingConcessionId() || isApprovingConcession()">
              {{ isApprovingConcession() ? 'Approving...' : 'Approve Concession' }}
            </button>
            @if (editingConcessionId()) {
              <button type="button" class="button button--secondary" (click)="resetApprovalForm()">Cancel</button>
            }
          </div>
        </form>
      </article>
    </div>

    @if (feedbackMessage(); as message) {
      <section class="notice" [class.notice--error]="feedbackTone() === 'error'">
        <strong>Fees update</strong>
        <p>{{ message }}</p>
      </section>
    }

    <div class="workspace-grid">
      <article class="data-card data-card--span">
        <div class="data-card__header">
          <div>
            <p class="eyebrow">Outstanding</p>
            <h3>Students pending collection</h3>
          </div>
        </div>
        <div class="application-list">
          @for (fee of store.feesDashboard()?.outstandingFees ?? []; track fee.id) {
            <article class="application-item">
              <div class="application-item__main">
                <div>
                  <strong>{{ fee.studentName }}</strong>
              <p>{{ fee.feeName }} - {{ fee.admissionNumber }}</p>
                </div>
                <span class="status-chip" [class]="'status-chip status-chip--' + statusTone(fee.status)">{{ fee.status }}</span>
              </div>
              <div class="application-meta">
                <span>Due {{ fee.dueOn | date: 'dd MMM yyyy' }}</span>
                <span>Amount {{ fee.amount | currency: 'INR':'symbol':'1.0-0' }}</span>
                <span>Concession {{ fee.concessionAmount | currency: 'INR':'symbol':'1.0-0' }}</span>
                <span>Balance {{ fee.balanceAmount | currency: 'INR':'symbol':'1.0-0' }}</span>
              </div>
            </article>
          }
        </div>
      </article>

      <article class="data-card">
        <div class="data-card__header">
          <div>
            <p class="eyebrow">Payments</p>
            <h3>Posted collection activity</h3>
          </div>
        </div>
        <div class="guardian-list">
          @for (payment of store.feePayments(); track payment.id) {
            <article class="guardian-item">
              <strong>{{ payment.studentName }}</strong>
              <p>{{ payment.feeName }} - {{ payment.paymentMethod }}</p>
              <p>{{ payment.paymentReference }} - {{ payment.paidOn | date: 'dd MMM yyyy' }}</p>
              <span>{{ payment.amount | currency: 'INR':'symbol':'1.0-0' }}</span>
              <div class="form-actions form-actions--inline">
                <button type="button" class="button button--secondary button--small" (click)="editPayment(payment)">Edit</button>
                <button type="button" class="button button--secondary button--small" (click)="viewReceipt(payment.id)">View receipt</button>
              </div>
            </article>
          }
        </div>
      </article>

      <article class="data-card">
        <div class="data-card__header">
          <div>
            <p class="eyebrow">Concessions</p>
            <h3>Approved reductions</h3>
          </div>
        </div>
        <div class="guardian-list">
          @for (concession of store.feeConcessions(); track concession.id) {
            <article class="guardian-item">
              <strong>{{ concession.studentName }}</strong>
              <p>{{ concession.feeName }} - {{ concession.concessionType }}</p>
              <p>{{ concession.approvedBy }} - {{ concession.approvedOn | date: 'dd MMM yyyy' }}</p>
              <span>{{ concession.amount | currency: 'INR':'symbol':'1.0-0' }}</span>
              <div class="form-actions form-actions--inline">
                <button type="button" class="button button--secondary button--small" (click)="editConcessionApproval(concession)">Approve</button>
              </div>
            </article>
          }
        </div>
      </article>

      <article class="data-card">
        <div class="data-card__header">
          <div>
            <p class="eyebrow">Structures</p>
            <h3>Configured billing heads</h3>
          </div>
        </div>
        <div class="guardian-list">
          @for (structure of store.feeStructures(); track structure.id) {
            <article class="guardian-item">
              <strong>{{ structure.feeName }}</strong>
              <p>{{ structure.className }} - {{ structure.billingCycle }}</p>
              <span>{{ structure.amount | currency: 'INR':'symbol':'1.0-0' }}</span>
            </article>
          }
        </div>
      </article>
    </div>

    @if (selectedReceipt(); as receipt) {
      <section class="data-card">
        <div class="data-card__header">
          <div>
            <p class="eyebrow">Receipt</p>
            <h3>Generated receipt preview</h3>
          </div>
        </div>
        <div class="guardian-list">
          <article class="guardian-item">
            <strong>{{ receipt.studentName }}</strong>
            <p>{{ receipt.feeName }} - {{ receipt.receiptNumber }}</p>
            <p>{{ receipt.paymentMethod }} - {{ receipt.paidOn | date: 'dd MMM yyyy' }}</p>
            <span>{{ receipt.amount | currency: 'INR':'symbol':'1.0-0' }}</span>
          </article>
        </div>
      </section>
    }
  `
})
export class FeesPageComponent {
  protected readonly store = inject(AppDataStore);
  protected readonly statusTone = (status: string) => status.toLowerCase().replace(/\s+/g, '-');
  protected readonly selectedReceipt = signal<FeeReceipt | null>(null);
  protected readonly editingPaymentId = signal<number | null>(null);
  protected readonly editingConcessionId = signal<number | null>(null);
  protected readonly isSubmittingPayment = signal(false);
  protected readonly isSubmittingConcession = signal(false);
  protected readonly isApprovingConcession = signal(false);
  protected readonly feedbackMessage = signal<string | null>(null);
  protected readonly feedbackTone = signal<'success' | 'error'>('success');
  protected readonly paymentMethods = ['Cash', 'Check', 'Online', 'Bank Transfer', 'UPI'];
  protected readonly concessionTypes = ['Merit', 'Financial Aid', 'Sibling', 'Sports', 'Other'];
  protected readonly today = computed(() => new Date().toISOString().split('T')[0]);

  private readonly fb = inject(FormBuilder);

  protected readonly paymentForm = this.fb.nonNullable.group({
    studentFeeId: ['', Validators.required],
    amount: [0, [Validators.required, Validators.min(0.01)]],
    paymentMethod: ['', Validators.required],
    paymentReference: [''],
    paidOn: [this.today(), Validators.required]
  });

  protected readonly concessionForm = this.fb.nonNullable.group({
    studentFeeId: ['', Validators.required],
    concessionType: ['', Validators.required],
    amount: [0, [Validators.required, Validators.min(0.01)]],
    remarks: ['']
  });

  protected readonly approvalForm = this.fb.nonNullable.group({
    approvedBy: ['', Validators.required]
  });

  protected onSubmitPayment(): void {
    if (this.paymentForm.invalid) {
      return;
    }

    this.isSubmittingPayment.set(true);
    this.feedbackMessage.set(null);
    const formValue = this.paymentForm.getRawValue();

    if (this.editingPaymentId()) {
      const dto: UpdateFeePayment = {
        amount: Number(formValue.amount),
        paymentMethod: formValue.paymentMethod,
        paymentReference: formValue.paymentReference,
        paidOn: formValue.paidOn
      };

      this.store.updateFeePayment(this.editingPaymentId()!, dto).subscribe({
        next: () => {
          this.isSubmittingPayment.set(false);
          this.feedbackTone.set('success');
          this.feedbackMessage.set('Fee payment updated successfully.');
          this.resetPaymentForm();
          this.store.loadFeesDashboard();
        },
        error: () => {
          this.isSubmittingPayment.set(false);
          this.feedbackTone.set('error');
          this.feedbackMessage.set('Fee payment could not be updated.');
        }
      });

      return;
    }

    const dto: RecordFeePayment = {
      studentFeeId: Number(formValue.studentFeeId),
      amount: Number(formValue.amount),
      paymentMethod: formValue.paymentMethod,
      paymentReference: formValue.paymentReference,
      paidOn: formValue.paidOn
    };

    this.store.recordFeePayment(dto).subscribe({
      next: (paymentId) => {
        this.isSubmittingPayment.set(false);
        this.feedbackTone.set('success');
        this.feedbackMessage.set('Fee payment recorded successfully.');
        this.resetPaymentForm();
        this.store.loadFeesDashboard();
        this.viewReceipt(paymentId);
      },
      error: () => {
        this.isSubmittingPayment.set(false);
        this.feedbackTone.set('error');
        this.feedbackMessage.set('Fee payment could not be recorded.');
      }
    });
  }

  protected onSubmitConcession(): void {
    if (this.concessionForm.invalid) {
      return;
    }

    this.isSubmittingConcession.set(true);
    this.feedbackMessage.set(null);
    const formValue = this.concessionForm.getRawValue();
    const dto: CreateFeeConcession = {
      studentFeeId: Number(formValue.studentFeeId),
      concessionType: formValue.concessionType,
      amount: Number(formValue.amount),
      remarks: formValue.remarks
    };

    this.store.createFeeConcession(dto).subscribe({
      next: () => {
        this.concessionForm.reset({
          studentFeeId: '',
          concessionType: '',
          amount: 0,
          remarks: ''
        });
        this.isSubmittingConcession.set(false);
        this.feedbackTone.set('success');
        this.feedbackMessage.set('Fee concession created successfully.');
        this.store.loadFeesDashboard();
      },
      error: () => {
        this.isSubmittingConcession.set(false);
        this.feedbackTone.set('error');
        this.feedbackMessage.set('Fee concession could not be created.');
      }
    });
  }

  protected onApproveConcession(): void {
    if (this.approvalForm.invalid || !this.editingConcessionId()) {
      return;
    }

    this.isApprovingConcession.set(true);
    this.feedbackMessage.set(null);
    const approvedBy = this.approvalForm.getRawValue().approvedBy.trim();

    this.store.approveFeeConcession(this.editingConcessionId()!, approvedBy).subscribe({
      next: () => {
        this.isApprovingConcession.set(false);
        this.feedbackTone.set('success');
        this.feedbackMessage.set('Fee concession approved successfully.');
        this.resetApprovalForm();
        this.store.loadFeesDashboard();
      },
      error: () => {
        this.isApprovingConcession.set(false);
        this.feedbackTone.set('error');
        this.feedbackMessage.set('Fee concession could not be approved.');
      }
    });
  }

  protected editPayment(payment: FeePayment): void {
    this.editingPaymentId.set(payment.id);
    this.paymentForm.reset({
      studentFeeId: '',
      amount: Number(payment.amount),
      paymentMethod: payment.paymentMethod,
      paymentReference: payment.paymentReference,
      paidOn: payment.paidOn
    });
  }

  protected editConcessionApproval(concession: FeeConcession): void {
    this.editingConcessionId.set(concession.id);
    this.approvalForm.reset({
      approvedBy: concession.approvedBy === 'Pending Approval' ? '' : concession.approvedBy
    });
  }

  protected resetPaymentForm(): void {
    this.editingPaymentId.set(null);
    this.paymentForm.reset({
      studentFeeId: '',
      amount: 0,
      paymentMethod: '',
      paymentReference: '',
      paidOn: this.today()
    });
  }

  protected resetApprovalForm(): void {
    this.editingConcessionId.set(null);
    this.approvalForm.reset({
      approvedBy: ''
    });
  }

  protected selectedPaymentLabel(): string {
    const payment = this.store.feePayments().find((item) => item.id === this.editingPaymentId());
    return payment ? `${payment.studentName} - ${payment.feeName}` : 'Selected payment';
  }

  protected selectedConcessionLabel(): string {
    const concession = this.store.feeConcessions().find((item) => item.id === this.editingConcessionId());
    return concession ? `${concession.studentName} - ${concession.feeName} - ${concession.concessionType}` : 'Select a concession from the list below';
  }

  protected viewReceipt(paymentId: number): void {
    this.store.generateFeeReceipt(paymentId).subscribe({
      next: (receipt) => {
        this.selectedReceipt.set(receipt);
      },
      error: () => {
        this.feedbackTone.set('error');
        this.feedbackMessage.set('Receipt could not be generated.');
      }
    });
  }
}
