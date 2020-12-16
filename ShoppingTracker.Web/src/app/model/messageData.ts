export interface MessageData {
    title?: string;
    message?: string;
    type: MessageType;
}

export enum MessageType {
    success,
    warning,
    error
}
