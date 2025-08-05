import type UserHeader from "./UserHeader";

export default interface Chat {
    id:string;
    title:string;
    description: string;
    imageUrl:string;
    members:UserHeader[]
}