import { Block } from 'src/app/models/block.model';
import { Zone } from "./zone.model";

export interface GetTMCSResponse {
    zones: Zone[];
    blocks: Block[];
}