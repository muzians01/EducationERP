import { Component, inject, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { AppDataStore } from '../app.data';
import { CreateTransportRoute, CreateTransportVehicle } from '../app.models';

@Component({
  selector: 'app-transport-page',
  imports: [FormsModule],
  template: `
    <section class="section-heading">
      <p class="eyebrow">Transport Module</p>
      <h2>Vehicle routes, fleet status, and capacity planning</h2>
    </section>

    @if (store.transportDashboard(); as dashboard) {
      <section class="workspace-grid">
        <article class="data-card">
          <div class="data-card__header">
            <div>
              <p class="eyebrow">Route Desk</p>
              <h3>{{ editingRouteId ? 'Update transport route' : 'Add a new route' }}</h3>
            </div>
          </div>
          <div class="form-grid">
            <label class="form-field">
              <span>Route name</span>
              <input type="text" [(ngModel)]="routeDraft.routeName" name="routeName" />
            </label>
            <label class="form-field">
              <span>Origin</span>
              <input type="text" [(ngModel)]="routeDraft.origin" name="routeOrigin" />
            </label>
            <label class="form-field">
              <span>Destination</span>
              <input type="text" [(ngModel)]="routeDraft.destination" name="routeDestination" />
            </label>
            <label class="form-field">
              <span>Status</span>
              <select [(ngModel)]="routeDraft.status" name="routeStatus">
                @for (status of routeStatuses; track status) {
                  <option [value]="status">{{ status }}</option>
                }
              </select>
            </label>
            <div class="form-actions">
              <button type="button" class="button button--primary" (click)="submitRoute()">
                {{ editingRouteId ? 'Update route' : 'Create route' }}
              </button>
              @if (editingRouteId) {
                <button type="button" class="button button--secondary" (click)="resetRouteDraft()">Cancel</button>
              }
            </div>
          </div>
        </article>

        <article class="data-card">
          <div class="data-card__header">
            <div>
              <p class="eyebrow">Vehicle Desk</p>
              <h3>{{ editingVehicleId ? 'Update fleet assignment' : 'Register a vehicle' }}</h3>
            </div>
          </div>
          <div class="form-grid">
            <label class="form-field">
              <span>Vehicle number</span>
              <input type="text" [(ngModel)]="vehicleDraft.vehicleNumber" name="vehicleNumber" />
            </label>
            <label class="form-field">
              <span>Vehicle type</span>
              <input type="text" [(ngModel)]="vehicleDraft.vehicleType" name="vehicleType" />
            </label>
            <label class="form-field">
              <span>Capacity</span>
              <input type="number" [(ngModel)]="vehicleDraft.capacity" name="vehicleCapacity" min="1" />
            </label>
            <label class="form-field">
              <span>Assigned route</span>
              <select [(ngModel)]="vehicleDraft.assignedRouteId" name="assignedRouteId">
                <option [ngValue]="0">Select route</option>
                @for (route of store.transportRoutes(); track route.id) {
                  <option [ngValue]="route.id">{{ route.routeName }}</option>
                }
              </select>
            </label>
            <label class="form-field">
              <span>Status</span>
              <select [(ngModel)]="vehicleDraft.status" name="vehicleStatus">
                @for (status of vehicleStatuses; track status) {
                  <option [value]="status">{{ status }}</option>
                }
              </select>
            </label>
            <div class="form-actions">
              <button type="button" class="button button--primary" (click)="submitVehicle()">
                {{ editingVehicleId ? 'Update vehicle' : 'Create vehicle' }}
              </button>
              @if (editingVehicleId) {
                <button type="button" class="button button--secondary" (click)="resetVehicleDraft()">Cancel</button>
              }
            </div>
          </div>
        </article>

        <article class="data-card">
          <div class="data-card__header">
            <div>
              <p class="eyebrow">Transport Overview</p>
              <h3>Fleet utilization and route health</h3>
            </div>
          </div>
          <div class="guardian-list">
            <article class="guardian-item">
              <strong>Routes</strong>
              <span>{{ dashboard.totalRoutes }}</span>
            </article>
            <article class="guardian-item">
              <strong>Vehicles</strong>
              <span>{{ dashboard.totalVehicles }}</span>
            </article>
            <article class="guardian-item">
              <strong>Active trips</strong>
              <span>{{ dashboard.activeTrips }}</span>
            </article>
            <article class="guardian-item">
              <strong>Capacity utilization</strong>
              <span>{{ dashboard.capacityUtilizationPercentage }}%</span>
            </article>
          </div>
        </article>

        <article class="data-card">
          <div class="data-card__header">
            <div>
              <p class="eyebrow">Current Routes</p>
              <h3>Planned route operations and edits</h3>
            </div>
          </div>
          <div class="application-list">
            @for (route of store.transportRoutes(); track route.id) {
              <article class="application-item">
                <div class="application-item__main">
                  <div>
                    <strong>{{ route.routeName }}</strong>
                    <p>{{ route.origin }} to {{ route.destination }}</p>
                  </div>
                  <span class="status-chip status-chip--{{ route.status.toLowerCase().replaceAll(' ', '-') }}">{{ route.status }}</span>
                </div>
                <div class="application-meta">
                  <span>{{ route.assignedVehicles }} vehicles</span>
                  <span>{{ route.activeTrips }} active trips</span>
                </div>
                <div class="form-actions form-actions--inline">
                  <button type="button" class="button button--secondary" (click)="editRoute(route.id)">Edit</button>
                  <button type="button" class="button button--danger" (click)="deleteRoute(route.id)">Delete</button>
                </div>
              </article>
            }
          </div>
        </article>

        <article class="data-card">
          <div class="data-card__header">
            <div>
              <p class="eyebrow">Fleet Inventory</p>
              <h3>Vehicles assigned to routes and active duty</h3>
            </div>
          </div>
          <div class="guardian-list">
            @for (vehicle of store.transportVehicles(); track vehicle.id) {
              <article class="guardian-item">
                <div class="guardian-item__main">
                  <div>
                    <strong>{{ vehicle.vehicleNumber }} ({{ vehicle.vehicleType }})</strong>
                    <p>{{ vehicle.assignedRoute }} - {{ vehicle.capacity }} seats</p>
                  </div>
                  <button type="button" class="button button--small button--secondary" (click)="editVehicle(vehicle.id)">Edit</button>
                </div>
                <span>{{ vehicle.status }}</span>
                <button type="button" class="button button--small button--danger" (click)="deleteVehicle(vehicle.id)">Delete</button>
              </article>
            }
          </div>
        </article>
      </section>
    } @else {
      <section class="notice notice--info">
        <strong>Transport data is loading.</strong>
        <p>Refresh the page if backend transport services are not available yet.</p>
      </section>
    }
  `
})
export class TransportPageComponent implements OnInit {
  protected readonly store = inject(AppDataStore);
  protected readonly routeStatuses = ['Active', 'Inactive', 'Under Maintenance'];
  protected readonly vehicleStatuses = ['On Route', 'Idle', 'Under Maintenance'];
  protected editingRouteId: number | null = null;
  protected editingVehicleId: number | null = null;
  protected routeDraft: CreateTransportRoute = this.createEmptyRouteDraft();
  protected vehicleDraft: CreateTransportVehicle = this.createEmptyVehicleDraft();

  ngOnInit(): void {
    if (!this.store.transportDashboard()) {
      this.store.loadTransportDashboard();
    }
  }

  protected submitRoute(): void {
    if (!this.routeDraft.routeName.trim() || !this.routeDraft.origin.trim() || !this.routeDraft.destination.trim()) {
      return;
    }

    const request = this.editingRouteId
      ? this.store.updateTransportRoute(this.editingRouteId, this.routeDraft)
      : this.store.createTransportRoute(this.routeDraft);

    request.subscribe({
      next: () => {
        this.resetRouteDraft();
        this.store.loadTransportDashboard();
      },
      error: (err) => {
        console.error('Failed to save transport route', err);
      }
    });
  }

  protected editRoute(routeId: number): void {
    const route = this.store.transportRoutes().find((item) => item.id === routeId);
    if (!route) {
      return;
    }

    this.editingRouteId = route.id;
    this.routeDraft = {
      routeName: route.routeName,
      origin: route.origin,
      destination: route.destination,
      status: route.status
    };
  }

  protected deleteRoute(routeId: number): void {
    this.store.deleteTransportRoute(routeId).subscribe({
      next: () => {
        if (this.editingRouteId === routeId) {
          this.resetRouteDraft();
        }

        this.store.loadTransportDashboard();
      },
      error: (err) => {
        console.error('Failed to delete transport route', err);
      }
    });
  }

  protected submitVehicle(): void {
    if (!this.vehicleDraft.vehicleNumber.trim() || !this.vehicleDraft.vehicleType.trim() || !this.vehicleDraft.assignedRouteId) {
      return;
    }

    const request = this.editingVehicleId
      ? this.store.updateTransportVehicle(this.editingVehicleId, this.vehicleDraft)
      : this.store.createTransportVehicle(this.vehicleDraft);

    request.subscribe({
      next: () => {
        this.resetVehicleDraft();
        this.store.loadTransportDashboard();
      },
      error: (err) => {
        console.error('Failed to save transport vehicle', err);
      }
    });
  }

  protected editVehicle(vehicleId: number): void {
    const vehicle = this.store.transportVehicles().find((item) => item.id === vehicleId);
    if (!vehicle) {
      return;
    }

    const route = this.store.transportRoutes().find((item) => item.routeName === vehicle.assignedRoute);
    this.editingVehicleId = vehicle.id;
    this.vehicleDraft = {
      vehicleNumber: vehicle.vehicleNumber,
      vehicleType: vehicle.vehicleType,
      capacity: vehicle.capacity,
      assignedRouteId: route?.id ?? 0,
      status: vehicle.status
    };
  }

  protected deleteVehicle(vehicleId: number): void {
    this.store.deleteTransportVehicle(vehicleId).subscribe({
      next: () => {
        if (this.editingVehicleId === vehicleId) {
          this.resetVehicleDraft();
        }

        this.store.loadTransportDashboard();
      },
      error: (err) => {
        console.error('Failed to delete transport vehicle', err);
      }
    });
  }

  protected resetRouteDraft(): void {
    this.editingRouteId = null;
    this.routeDraft = this.createEmptyRouteDraft();
  }

  protected resetVehicleDraft(): void {
    this.editingVehicleId = null;
    this.vehicleDraft = this.createEmptyVehicleDraft();
  }

  private createEmptyRouteDraft(): CreateTransportRoute {
    return {
      routeName: '',
      origin: '',
      destination: '',
      status: 'Active'
    };
  }

  private createEmptyVehicleDraft(): CreateTransportVehicle {
    return {
      vehicleNumber: '',
      vehicleType: '',
      capacity: 40,
      assignedRouteId: 0,
      status: 'Idle'
    };
  }
}
