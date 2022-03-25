import { SMS } from "./sms.model";

export interface GetSMSResponse {
    smSs: SMS[];
    total: number;
}