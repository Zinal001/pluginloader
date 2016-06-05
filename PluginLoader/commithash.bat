@echo off
git log --pretty^=format:%%h -n 1 > commithash.txt