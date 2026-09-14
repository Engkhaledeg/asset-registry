import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, shareReplay } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Location } from '../models/location.model';

@Injectable({ providedIn: 'root' })
export class LocationApiService {
  private readonly locations$: Observable<Location[]>;

  constructor(private readonly http: HttpClient) {
    this.locations$ = this.http
      .get<Location[]>(`${environment.apiBaseUrl}/locations`)
      .pipe(shareReplay({ bufferSize: 1, refCount: false }));
  }

  getAll(): Observable<Location[]> {
    return this.locations$;
  }
}
