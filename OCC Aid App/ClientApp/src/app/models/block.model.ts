import { Zone } from "./zone.model";

export interface Block {
    blockId?: number
    name: string;
    startLength: number;
    endLength: number;
    shaftName: string;
    zoneId?: number;
    ext2: number;
}