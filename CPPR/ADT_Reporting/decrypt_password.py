# -*- coding: utf-8 -*-
"""
Created on Thu Jun 15 07:22:52 2023

@author: squack1
"""

def decrypt_password(u):
    import  rsa
    
    #usr = os.getlogin()    
    usr = u
    
    #READ ENCODED PASSWORD
    file = 'C:/Users/' + usr +'/Documents/ms_pwd.txt'
    f = open(file, 'rb')
    EncodedPassword = f.read()
    f.close()
    
    #READ PRIVATE KEY
    file = 'C:/Users/' + usr +'/Documents/ms_priv_key.txt'
    f = open(file, 'rb')
    PrivateKey = f.read()
    f.close()
    
    PrivateKey2 = rsa.PrivateKey.load_pkcs1(PrivateKey)
        
    DecryptedPassword = rsa.decrypt(EncodedPassword, PrivateKey2).decode()
    
    return DecryptedPassword
    