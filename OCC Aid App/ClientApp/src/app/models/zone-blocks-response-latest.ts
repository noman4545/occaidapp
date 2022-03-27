import { BlockLatest } from "./block-latest.model";

export interface ZoneBlocksResponseLatest {
    id?: number;
    blockId: number;
    zoneId: number;
    isDeleted: boolean;
    createdDate: Date;
    deletedDate: Date;
    modifyDate: Date;
    block: BlockLatest;
}