create schema NilArea;
create user 'nilorleans'@'%' identified by '9b6b00c9b2b312b2734bb9b781607b8b4730b6eda49026caadd880af4c6abf5d';
grant all privileges on NilArea.* to 'nilorleans'@'%' with grant option;
flush privileges;
