Unity & C# Save/Load System 

Lightweight Unity save/load system for player, inventory, and interactable objects with Auto Save and XOR-based JSON encryption.

Features
- Player position & rotation  
- Inventory keys  
- Door/Drawer states  
- Manual (F5/F9) & Auto Save (F6)  
- Save icon UI (optional)  
- Old save cleanup (F12)

Modular & Cross-Platform
- Easy to integrate  
- PC, Mac, Android, iOS, WebGL  
- Safe storage with `persistentDataPath`

Security
- XOR-encrypted JSON saves  
- Prevents casual tampering  
- Not suitable for sensitive data

Controls
- **F5** → Save  
- **F9** → Load  
- **F12** → Delete save  
- **F6** → Toggle Auto Save

> Save file: `Application.persistentDataPath/savegame.json`


© 2026 Erenkaragozz .. All Rights Reserved.  
This code is for portfolio purposes only. Do not copy, use, or distribute without permission.