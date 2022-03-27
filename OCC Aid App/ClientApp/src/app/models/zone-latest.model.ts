import { BlockLatest } from "./block-latest.model";

export interface ZoneLatest {
    id?: number;
    name: string;
    fanDirection: string;
    cctvLayout: string;
    zoneLayout: string;
    upName: string;
    leftName: string;
    rightName: string;
    shaftName: string;
    blocks: BlockLatest[];
}