import { Block } from "./block.model";

export interface Zone {
    zoneId?: number;
    name: string;
    fanDirection: string;
    cctvLayout: string;
    zoneLayout:string;
    upName: string;
    leftName: string;
    rightName:string;
    trackNo: string;
    blocks: Block[];
}