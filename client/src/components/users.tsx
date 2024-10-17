export interface User {
    name: string;
    profileImage: string;
    bodyImages: {url: string; date: string;}[];
}
  
  // Define and export the users array
  export const users: User[] = [
    { 
      name: 'Baifan', 
      profileImage: 'https://picsum.photos/102', 
      bodyImages: [
        { url: 'https://picsum.photos/450', date: '2024-10-1' },
        { url: 'https://picsum.photos/300/201', date: '2024-10-6' },
        { url: 'https://picsum.photos/300/202', date: '2024-10-30' }
      ] 
    },
    { 
      name: 'Raifan', 
      profileImage: 'https://picsum.photos/50', 
      bodyImages: [
        { url: 'https://picsum.photos/300/203', date: '2024-10-2' },
        { url: 'https://picsum.photos/300/204', date: '2024-10-19' }
      ] 
    },
    { 
      name: 'Saifan', 
      profileImage: 'https://picsum.photos/51', 
      bodyImages: [
        { url: 'https://picsum.photos/300/205', date: '2024-10-08' }
      ] 
    },
    { 
      name: 'Taifan', 
      profileImage: 'https://picsum.photos/52', 
      bodyImages: [
        { url: 'https://picsum.photos/300/205', date: '2024-10-06' },
        { url: 'https://picsum.photos/300/206', date: '2024-10-26' },
        { url: 'https://picsum.photos/300/207', date: '2024-10-11' }
      ] 
    },
    { 
      name: 'Taifan 2 electricbogaloo', 
      profileImage: '', 
      bodyImages: [{ url: '', date: '' }] 
    }
  ];
  