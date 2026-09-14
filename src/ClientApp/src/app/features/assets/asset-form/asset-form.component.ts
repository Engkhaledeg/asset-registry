import { Component, DestroyRef, OnInit } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { AssetApiService } from '../../../core/api/asset-api.service';
import { LocationApiService } from '../../../core/api/location-api.service';
import { Asset, AssetStatus, assetStatusLabels } from '../../../core/models/asset.model';
import { Location } from '../../../core/models/location.model';

@Component({
  selector: 'ar-asset-form',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './asset-form.component.html',
  styleUrl: './asset-form.component.scss'
})
export class AssetFormComponent implements OnInit {
  readonly form = this.formBuilder.nonNullable.group({
    assetTag: ['', [Validators.required, Validators.maxLength(20)]],
    name: ['', [Validators.required, Validators.maxLength(150)]],
    manufacturer: ['', [Validators.required, Validators.maxLength(100)]],
    serialNumber: ['', [Validators.required, Validators.maxLength(100)]],
    purchasedOn: ['', Validators.required],
    purchasePrice: [0, [Validators.required, Validators.min(0)]],
    locationId: ['', Validators.required],
    status: 'InStock' as AssetStatus
  });

  readonly statusOptions = (Object.keys(assetStatusLabels) as AssetStatus[]).map((status) => ({
    value: status,
    label: assetStatusLabels[status]
  }));

  locations: Location[] = [];
  assetId: number | null = null;
  isSaving = false;
  errorMessage: string | null = null;

  constructor(
    private readonly assetApi: AssetApiService,
    private readonly locationApi: LocationApiService,
    private readonly formBuilder: FormBuilder,
    private readonly route: ActivatedRoute,
    private readonly router: Router,
    private readonly destroyRef: DestroyRef
  ) {}

  get isEditing(): boolean {
    return this.assetId !== null;
  }

  get title(): string {
    return this.isEditing ? 'Edit asset' : 'Add asset';
  }

  ngOnInit(): void {
    this.locationApi
      .getAll()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((locations) => (this.locations = locations));

    const routeId = this.route.snapshot.paramMap.get('id');

    if (routeId !== null) {
      this.assetId = Number(routeId);
      this.form.controls.assetTag.disable();
      this.loadAsset(this.assetId);
    }
  }

  save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.isSaving = true;
    this.errorMessage = null;

    const request = this.form.getRawValue();
    const payload = {
      name: request.name,
      manufacturer: request.manufacturer,
      serialNumber: request.serialNumber,
      purchasedOn: request.purchasedOn,
      purchasePrice: Number(request.purchasePrice),
      locationId: Number(request.locationId)
    };

    const save$ = this.isEditing
      ? this.assetApi.update(this.assetId!, { ...payload, status: request.status })
      : this.assetApi.create({ ...payload, assetTag: request.assetTag });

    save$.pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: () => this.router.navigate(['/assets']),
      error: (error: HttpErrorResponse) => {
        this.errorMessage = this.readErrorDetail(error);
        this.isSaving = false;
      }
    });
  }

  private loadAsset(id: number): void {
    this.assetApi
      .getById(id)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (asset) => this.fillForm(asset),
        error: () => (this.errorMessage = 'That asset could not be loaded. It may have been deleted.')
      });
  }

  private fillForm(asset: Asset): void {
    this.form.setValue({
      assetTag: asset.assetTag,
      name: asset.name,
      manufacturer: asset.manufacturer,
      serialNumber: asset.serialNumber,
      purchasedOn: asset.purchasedOn,
      purchasePrice: asset.purchasePrice,
      locationId: String(asset.locationId),
      status: asset.status
    });
  }

  private readErrorDetail(error: HttpErrorResponse): string {
    const detail = error.error?.detail;

    return typeof detail === 'string' && detail.length > 0
      ? detail
      : 'The asset could not be saved. Check the values and try again.';
  }
}
