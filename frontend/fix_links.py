import os
import glob
import re

routes_map = {
    'dashboard': '/dashboard',
    'receipt_long': '/payments',
    'group': '/users',
    'inventory_2': '/inventory',
    'payments': '/revenue',  # for Doanh thu if present
    'login': '/login',
    'register': '/register',
    'store': '/'
}

# we will replace href="#" with @click.prevent="$router.push('...')"
for file_path in glob.glob('src/screens/*.vue'):
    with open(file_path, 'r', encoding='utf-8') as f:
        content = f.read()
    
    # Simple regex to find <a> tags inside <aside> or header, and inject click handlers
    # Actually, let's just find <a> tags that contain the specific data-icon and replace their href="#"
    
    for icon, route in routes_map.items():
        # Match <a ...> ... data-icon="icon" ... </a>
        # This regex is a bit complex. Let's do string replacement instead.
        # Find blocks of <a ... </a>
        def repl(match):
            a_tag_content = match.group(0)
            if f'data-icon="{icon}"' in a_tag_content or f'>{icon}<' in a_tag_content:
                return a_tag_content.replace('href="#"', f'href="{route}" @click.prevent="$router.push(\'{route}\')"')
            return a_tag_content
            
        content = re.sub(r'<a[^>]*>.*?</a>', repl, content, flags=re.DOTALL)
        
    # Handle Storefront specific links if they exist
    content = content.replace('href="#" @click.prevent="$router.push(\'/login\')"', 'href="/login" @click.prevent="$router.push(\'/login\')"')

    # Also fix some specific text links
    content = content.replace('>Đăng nhập ngay<', ' @click.prevent="$router.push(\'/login\')">Đăng nhập ngay<')
    content = content.replace('>Đăng ký ngay<', ' @click.prevent="$router.push(\'/register\')">Đăng ký ngay<')
    
    with open(file_path, 'w', encoding='utf-8') as f:
        f.write(content)

print("Updated links in all Vue files.")
