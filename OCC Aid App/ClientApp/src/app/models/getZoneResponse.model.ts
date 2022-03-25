import { Zone } from "./zone.model";

export interface GetZoneResponse {
    zones: Zone[];
    total: number;
}