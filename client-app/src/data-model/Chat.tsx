import type UserHeader from "./UserHeader";

export const ChatType = {
    MONOLOGUE: 1,
    DIALOGUE: 2,
    POLYLOGUE: 3,
} as const;

export type ChatType = typeof ChatType[keyof typeof ChatType];

export default interface Chat {
    id:string,
    title:string,
    type: ChatType,
    description: string,
    imageId:string,
    members:UserHeader[]
    currentPermissions:{
        canEdit:boolean,
        canDelete:boolean,
        canAddMember:boolean,
        canPromote:boolean,
        canRemoveMember:boolean,
        canSendMessages:boolean
    }
}
