import { IOSCode } from "./iosCode.model";

export interface GetIOSCodesResponse {
    iosCodes: IOSCode[];
    total: number;
}