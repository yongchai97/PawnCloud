
import re
with open('src/shared/service-proxies/service-proxies.ts', 'r', encoding='utf-8') as f:
    content = f.read()

# Remove the misplaced helper block
wrong = chr(10).join([
    '    });',
    '',
    'function unwrapAbpEnvelope(data) {',
    '    if (data && typeof data === \'object\' && data.__abp && \'result\' in data) {',
    '        return data.result;',
    '    }',
    '    return data;',
    '}',
    '',
    '}',
])
correct = chr(10).join(['    });', '}'])
if wrong in content:
    content = content.replace(wrong, correct, 1)
    print('Removed misplaced helper')
else:
    print('Misplaced helper block not found exactly')
