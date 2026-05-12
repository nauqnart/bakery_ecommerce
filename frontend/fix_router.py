import os
import re

directory = r'd:\stitch_artisan_bakery_shop\frontend\src\screens'

for filename in os.listdir(directory):
    if filename.endswith('.vue'):
        filepath = os.path.join(directory, filename)
        with open(filepath, 'r', encoding='utf-8') as f:
            content = f.read()
        
        # We need to replace the <a> blocks that contain @click.prevent="$router.push
        # Find all such blocks
        pattern = re.compile(r'<a([^>]*?)href="([^"]+)"\s*@click\.prevent="\$router\.push\([^)]+\)"([^>]*?)>(.*?)</a>', re.DOTALL)
        
        new_content = pattern.sub(r'<router-link\1to="\2"\3>\4</router-link>', content)
        
        if new_content != content:
            with open(filepath, 'w', encoding='utf-8') as f:
                f.write(new_content)
            print(f'Fixed {filename}')
