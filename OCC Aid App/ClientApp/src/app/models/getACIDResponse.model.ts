import { ACID } from "./acid.model";

export interface GetACIDResponse {
    acids: ACID[];
    total: number;
}