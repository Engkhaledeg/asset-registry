import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Asset, AssetSearchQuery, CreateAssetRequest, UpdateAssetRequest } from '../models/asset.model';
import { PagedResult } from '../models/paged-result.model';

@Injectable({ providedIn: 'root' })
export class AssetApiService {
  private readonly endpoint = `${environment.apiBaseUrl}/assets`;

  constructor(private readonly http: HttpClient) {}

  search(query: AssetSearchQuery): Observable<PagedResult<Asset>> {
    return this.http.get<PagedResult<Asset>>(this.endpoint, { params: this.toHttpParams(query) });
  }

  getById(id: number): Observable<Asset> {
    return this.http.get<Asset>(`${this.endpoint}/${id}`);
  }

  create(request: CreateAssetRequest): Observable<Asset> {
    return this.http.post<Asset>(this.endpoint, request);
  }

  update(id: number, request: UpdateAssetRequest): Observable<Asset> {
    return this.http.put<Asset>(`${this.endpoint}/${id}`, request);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.endpoint}/${id}`);
  }

  private toHttpParams(query: AssetSearchQuery): HttpParams {
    let params = new HttpParams()
      .set('page', query.page)
      .set('pageSize', query.pageSize);

    if (query.searchTerm.trim().length > 0) {
      params = params.set('searchTerm', query.searchTerm.trim());
    }

    if (query.status !== '') {
      params = params.set('status', query.status);
    }

    if (query.locationId !== '') {
      params = params.set('locationId', query.locationId);
    }

    return params;
  }
}
