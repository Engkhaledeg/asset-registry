import { Component, Input } from '@angular/core';
import { AssetStatus, assetStatusLabels } from '../../core/models/asset.model';

@Component({
  selector: 'ar-status-badge',
  standalone: true,
  templateUrl: './status-badge.component.html',
  styleUrl: './status-badge.component.scss'
})
export class StatusBadgeComponent {
  @Input({ required: true }) status!: AssetStatus;

  get label(): string {
    return assetStatusLabels[this.status];
  }
}
