import { CurrencyPipe, DatePipe } from '@angular/common';
import { Component, DestroyRef, OnInit } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { debounceTime, distinctUntilChanged } from 'rxjs';
import { AssetApiService } from '../../../core/api/asset-api.service';
import { LocationApiService } from '../../../core/api/location-api.service';
import { Asset, AssetStatus, assetStatusLabels } from '../../../core/models/asset.model';
import { Location } from '../../../core/models/location.model';
import { StatusBadgeComponent } from '../../../shared/status-badge/status-badge.component';

const pageSize = 20;

@Component({
  selector: 'ar-asset-list',
  standalone: true,
  imports: [CurrencyPipe, DatePipe, ReactiveFormsModule, RouterLink, StatusBadgeComponent],
  templateUrl: './asset-list.component.html',
  styleUrl: './asset-list.component.scss'
})
export class AssetListComponent implements OnInit {
  readonly filters = this.formBuilder.nonNullable.group({
    searchTerm: '',
    status: '' as AssetStatus | '',
    locationId: ''
  });

  readonly statusOptions = (Object.keys(assetStatusLabels) as AssetStatus[]).map((status) => ({
    value: status,
    label: assetStatusLabels[status]
  }));

  locations: Location[] = [];
  assets: Asset[] = [];
  totalCount = 0;
  totalPages = 0;
  page = 1;
  isLoading = false;
  errorMessage: string | null = null;

  constructor(
    private readonly assetApi: AssetApiService,
    private readonly locationApi: LocationApiService,
    private readonly formBuilder: FormBuilder,
    private readonly destroyRef: DestroyRef
  ) {}

  ngOnInit(): void {
    this.locationApi
      .getAll()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((locations) => (this.locations = locations));

    this.filters.valueChanges
      .pipe(debounceTime(300), distinctUntilChanged(this.filtersAreEqual), takeUntilDestroyed(this.destroyRef))
      .subscribe(() => this.goToPage(1));

    this.loadAssets();
  }

  goToPage(page: number): void {
    this.page = page;
    this.loadAssets();
  }

  deleteAsset(asset: Asset): void {
    const confirmed = window.confirm(`Delete ${asset.assetTag} - ${asset.name}?`);

    if (!confirmed) {
      return;
    }

    this.assetApi
      .delete(asset.id)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => this.loadAssets(),
        error: () => (this.errorMessage = 'That asset could not be deleted. Refresh and try again.')
      });
  }

  private loadAssets(): void {
    this.isLoading = true;
    this.errorMessage = null;

    const { searchTerm, status, locationId } = this.filters.getRawValue();

    this.assetApi
      .search({ searchTerm, status, locationId, page: this.page, pageSize })
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (result) => {
          this.assets = result.items;
          this.totalCount = result.totalCount;
          this.totalPages = result.totalPages;
          this.isLoading = false;
        },
        error: () => {
          this.errorMessage = 'The asset list could not be loaded. Check your connection and try again.';
          this.isLoading = false;
        }
      });
  }

  private filtersAreEqual(previous: unknown, current: unknown): boolean {
    return JSON.stringify(previous) === JSON.stringify(current);
  }
}
