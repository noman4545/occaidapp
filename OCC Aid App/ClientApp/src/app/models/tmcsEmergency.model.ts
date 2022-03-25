import { Block } from "./block.model";
import { Zone } from "./zone.model";

export interface TMCSEmergency {
    id?: number;
    zoneId: number;
    blockId: number;
    completed: boolean;
    dmDecision: string;
    createdDate: Date;
    zone: Zone;
    block: Block;
}