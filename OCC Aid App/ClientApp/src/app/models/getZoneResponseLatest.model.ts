import { ZoneResponseLatest } from "./zone-response-latest.model";

export interface GetZoneResponseLatest {
    zones: ZoneResponseLatest[];
    total: number;
}