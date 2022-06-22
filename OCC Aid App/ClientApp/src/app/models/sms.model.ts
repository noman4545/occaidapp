export interface SMS {
    id?: number;
    typeOfFailure: string;
    systemBehaviour: string;
    workInstruction: string;
    message: string;
    timeToReturnToTimetable: string;
    completed: boolean;
    isRequiredDmReview: boolean;
    isDmReviewed: boolean;
}