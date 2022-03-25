import { Log } from "./log.model";

export interface GetLogsResponse {
    logs: Log[];
    total: number;
}