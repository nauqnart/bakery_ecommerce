import urllib.request
import re
import os

screens = [
    {"name": "InventoryControl", "url": "https://contribution.usercontent.google.com/download?c=CgthaWRhX2NvZGVmeBJ6Eh1hcHBfY29tcGFuaW9uX2dlbmVyYXRlZF9maWxlcxpZCiVodG1sXzlkMDBjNTVlODU1NTRjNGQ5MTVhNDI5ZDYxNzY4NWViEgoSBhD03se7WxgBkgEjCgpwcm9qZWN0X2lkEhVCEzQ2Mjc5NzA2MjQ4NDQ2Mzk2NTA&filename=&opi=89354086"},
    {"name": "UserManagement", "url": "https://contribution.usercontent.google.com/download?c=CgthaWRhX2NvZGVmeBJ6Eh1hcHBfY29tcGFuaW9uX2dlbmVyYXRlZF9maWxlcxpZCiVodG1sX2E3NThmYWM2ZmM3OTQxNWE4NWY4OGE1OTEyZjcxZTVhEgoSBhD03se7WxgBkgEjCgpwcm9qZWN0X2lkEhVCEzQ2Mjc5NzA2MjQ4NDQ2Mzk2NTA&filename=&opi=89354086"},
    {"name": "Storefront", "url": "https://contribution.usercontent.google.com/download?c=CgthaWRhX2NvZGVmeBJ6Eh1hcHBfY29tcGFuaW9uX2dlbmVyYXRlZF9maWxlcxpZCiVodG1sX2VkMTVkOWZkZWE0MzQyOGZiNTJhYjMwYjgzN2RkMmZjEgoSBhD03se7WxgBkgEjCgpwcm9qZWN0X2lkEhVCEzQ2Mjc5NzA2MjQ4NDQ2Mzk2NTA&filename=&opi=89354086"},
    {"name": "RevenueDashboard", "url": "https://contribution.usercontent.google.com/download?c=CgthaWRhX2NvZGVmeBJ6Eh1hcHBfY29tcGFuaW9uX2dlbmVyYXRlZF9maWxlcxpZCiVodG1sX2FhYjI4MjI2YjE1MzQ3ZjhhMGZlMDIxMzY4NjM1ZWVkEgoSBhD03se7WxgBkgEjCgpwcm9qZWN0X2lkEhVCEzQ2Mjc5NzA2MjQ4NDQ2Mzk2NTA&filename=&opi=89354086"},
    {"name": "PaymentHistory", "url": "https://contribution.usercontent.google.com/download?c=CgthaWRhX2NvZGVmeBJ6Eh1hcHBfY29tcGFuaW9uX2dlbmVyYXRlZF9maWxlcxpZCiVodG1sXzM3NTBkNmFiMjBkMTQ0YWY4Zjg2OGI0YjRlODQ0YjIyEgoSBhD03se7WxgBkgEjCgpwcm9qZWN0X2lkEhVCEzQ2Mjc5NzA2MjQ4NDQ2Mzk2NTA&filename=&opi=89354086"},
    {"name": "Register", "url": "https://contribution.usercontent.google.com/download?c=CgthaWRhX2NvZGVmeBJ6Eh1hcHBfY29tcGFuaW9uX2dlbmVyYXRlZF9maWxlcxpZCiVodG1sXzUzYWNkMjg5ZDdkMzQxM2JhNmI4M2Y1Y2M5OGExNDBjEgoSBhD03se7WxgBkgEjCgpwcm9qZWN0X2lkEhVCEzQ2Mjc5NzA2MjQ4NDQ2Mzk2NTA&filename=&opi=89354086"},
    {"name": "Login", "url": "https://contribution.usercontent.google.com/download?c=CgthaWRhX2NvZGVmeBJ6Eh1hcHBfY29tcGFuaW9uX2dlbmVyYXRlZF9maWxlcxpZCiVodG1sXzAxY2UzZjczNjkwOTRjNDk5M2Q1YjViYmE3ZTU4MTdjEgoSBhD03se7WxgBkgEjCgpwcm9qZWN0X2lkEhVCEzQ2Mjc5NzA2MjQ4NDQ2Mzk2NTA&filename=&opi=89354086"},
    {"name": "Dashboard", "url": "https://contribution.usercontent.google.com/download?c=CgthaWRhX2NvZGVmeBJ6Eh1hcHBfY29tcGFuaW9uX2dlbmVyYXRlZF9maWxlcxpZCiVodG1sX2YxOGUxODhkMzA0NTRlYmJiNjRhOWFjMGVjOWU2YjAwEgoSBhD03se7WxgBkgEjCgpwcm9qZWN0X2lkEhVCEzQ2Mjc5NzA2MjQ4NDQ2Mzk2NTA&filename=&opi=89354086"}
]

os.makedirs('src/screens', exist_ok=True)

for screen in screens:
    req = urllib.request.Request(screen['url'], headers={'User-Agent': 'Mozilla/5.0'})
    try:
        with urllib.request.urlopen(req) as response:
            html = response.read().decode('utf-8')
            
            # Extract body content
            body_match = re.search(r'<body[^>]*>(.*?)</body>', html, re.DOTALL)
            body_content = body_match.group(1) if body_match else ""
            
            # Extract tailwind script if exists
            tailwind_match = re.search(r'<script src="https://cdn\.tailwindcss\.com.*?"></script>', html)
            tailwind_script = tailwind_match.group(0) if tailwind_match else ""
            
            # Extract styles
            style_match = re.search(r'<style>(.*?)</style>', html, re.DOTALL)
            style_content = style_match.group(1) if style_match else ""
            
            vue_content = f"""<template>
  <div class="{screen['name'].lower()}">
    {body_content}
  </div>
</template>

<script setup>
</script>

<style scoped>
{style_content}
</style>
"""
            with open(f"src/screens/{screen['name']}.vue", "w", encoding="utf-8") as f:
                f.write(vue_content)
            print(f"Created {screen['name']}.vue")
    except Exception as e:
        print(f"Error downloading {screen['name']}: {e}")
