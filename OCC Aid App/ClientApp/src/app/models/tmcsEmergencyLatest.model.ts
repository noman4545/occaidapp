import { BlockLatest } from "./block-latest.model";
import { ZoneBlocksResponseLatest } from "./zone-blocks-response-latest";
import { ZoneResponseLatest } from "./zone-response-latest.model";

export interface TMCSEmergencyLatest {
    id?: number;
    zoneId: number;
    blockId: number;
    completed: boolean;
    efcMarkedCompleted: boolean;
    dmDecision: string;
    createdDate: Date;
    zone: ZoneResponseLatest;
    block: BlockLatest;
}