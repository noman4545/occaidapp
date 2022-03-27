import { ZoneBlocksResponseLatest } from "./zone-blocks-response-latest";

export interface ZoneResponseLatest {
    id?: number;
    name: string;
    fanDirection: string;
    cctvLayout: string;
    zoneLayout: string;
    upName: string;
    leftName: string;
    rightName: string;
    shaftName: string;
    createdDate: Date;
    deletedDate: Date;
    modifyDate: Date;
    zoneBlocks: ZoneBlocksResponseLatest[];
}