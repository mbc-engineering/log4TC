import hashlib
import uuid
import base64

#system_id = '5C78FAE1-9512-4E8F-F704-5A4DFF9D86BD'
system_id = input("System-ID:")
license_id = '9BDDB874-8F39-4D26-90BB-4237ED0644FB'

sys_id = uuid.UUID(system_id).bytes_le
lic_id = uuid.UUID(license_id).bytes_le

h = hashlib.sha256()
h.update(sys_id)
h.update(lic_id)
hash = h.digest()

print(base64.b64encode(hash[0:30]))

