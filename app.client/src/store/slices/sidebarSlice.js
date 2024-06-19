import WhatsAppIcon from '@mui/icons-material/WhatsApp';
import TelegramIcon from '@mui/icons-material/Telegram';
import YoutubeIcon from '@mui/icons-material/YouTube';
import { createSlice } from '@reduxjs/toolkit';

const initialState = {
    social: [
        { name: 'Telegram', icon: TelegramIcon },
        { name: 'WhatsApp', icon: WhatsAppIcon },    
        { name: 'Youtube', icon: YoutubeIcon }
    ],
};

const sidebarSlice = createSlice({
  name: 'sidebar',
  initialState,
  reducers: {},
});

export default sidebarSlice.reducer;
