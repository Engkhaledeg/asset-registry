export type AssetStatus = 'InStock' | 'Deployed' | 'UnderRepair' | 'Retired';

export interface Asset {
  id: number;
  assetTag: string;
  name: string;
  manufacturer: string;
  serialNumber: string;
  purchasedOn: string;
  purchasePrice: number;
  status: AssetStatus;
  locationId: number;
  locationName: string;
}

export interface CreateAssetRequest {
  assetTag: string;
  name: string;
  manufacturer: string;
  serialNumber: string;
  purchasedOn: string;
  purchasePrice: number;
  locationId: number;
}

export interface UpdateAssetRequest extends Omit<CreateAssetRequest, 'assetTag'> {
  status: AssetStatus;
}

export interface AssetSearchQuery {
  searchTerm: string;
  status: AssetStatus | '';
  locationId: string;
  page: number;
  pageSize: number;
}

export const assetStatusLabels: Record<AssetStatus, string> = {
  InStock: 'In stock',
  Deployed: 'Deployed',
  UnderRepair: 'Under repair',
  Retired: 'Retired'
};
